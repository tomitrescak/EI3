

using Ei.Ontology;
using Ei.Persistence.Actions;
using Ei.Persistence.Transitions;
using Ei.Runtime;

namespace Ei.Data.Yaml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using YamlDotNet.Serialization;
    using System.IO;
    using Ei.Persistence;


    //    class TC : IYamlTypeConverter
    //    {
    //        public bool Accepts(Type type)
    //        {
    //            throw new NotImplementedException();
    //        }
    //
    //        public object ReadYaml(IParser parser, Type type)
    //        {
    //            throw new NotImplementedException();
    //        }
    //
    //        public void WriteYaml(IEmitter emitter, object value, Type type)
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    struct WorkflowConnection
    {
        internal ConnectionDao Connection;
        internal WorkflowDao Workflow;

        public WorkflowConnection(ConnectionDao connection, WorkflowDao workflow)
        {
            this.Connection = connection;
            this.Workflow = workflow;
        }
    }

    public class YamlInstitutionFactory : IDaoFactory
    {
        private static YamlInstitutionFactory instance;

        public static YamlInstitutionFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new YamlInstitutionFactory();
                }
                return instance;
            }

        }

        public InstitutionDao LoadFromFile(string path)
        {
            //this.path = path;
            var directory = Path.GetDirectoryName(path);

            return this.Load(File.ReadAllText(path), new WorkflowImportPathLoader(directory));
        }

        public InstitutionDao Load(string source, IWorkflowImportLoader importLoader)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("Source is empty");
            }

            var dao = this.Deserialise<InstitutionDao>(source); // deserializer.Deserialize<InstitutionDao>(input);

            // now resolve all imports
            for (var i = dao.Workflows.Count - 1; i >= 0; i--)
            {
                var workflowDao = dao.Workflows[i];

                if (!string.IsNullOrEmpty(workflowDao.Import))
                {
                    // serialise import
                    var importText = importLoader.LoadImport(workflowDao.Import);
                    var workflowImport = this.Deserialise<WorkflowImportDao>(importText);

                    if (dao.Roles == null) dao.Roles = new List<RoleDao>();
                    if (dao.Organisations == null) dao.Organisations = new List<OrganisationDao>();
                    if (dao.Types == null) dao.Types = new List<ClassDao>();

                    // merge all organisations
                    this.Merge(workflowImport.Roles, dao.Roles);
                    this.Merge(workflowImport.Organisations, dao.Organisations);
                    this.Merge(workflowImport.Types, dao.Types);

                    // remove the original workflow (import carrier)
                    dao.Workflows.Remove(workflowDao);

                    // import the workflow, assign the same id to it
                    var importedWorkflow = workflowImport.Workflow;
                    importedWorkflow.Id = workflowDao.Id;
                    dao.Workflows.Add(importedWorkflow);

                    // add parameters which may have been overriden
                    foreach (var parameterDao in workflowDao.Properties)
                    {
                        importedWorkflow.Properties.RemoveAll(w => w.Name == parameterDao.Name);
                        importedWorkflow.Properties.Add(parameterDao);
                    }
                }
            }

            // for each join workflow action extract the list of postconditions

            if (!string.IsNullOrEmpty(dao.InitialWorkflow))
            {
                foreach (var wconn in ExtractWorkflowConnections(dao, dao.Workflows.First(w => w.Id == dao.InitialWorkflow)))
                {
                    ExtractEffects(dao, wconn);
                }
            }

            // move all functions into global list

            foreach (var workflow in dao.Workflows)
            {
                if (workflow.Functions != null)
                {
                    // init properties if necessary
                    if (dao.Globals == null) dao.Globals = new GlobalsDao();
                    if (dao.Globals.Functions == null) dao.Globals.Functions = new List<FunctionDao>();

                    dao.Globals.Functions.AddRange(workflow.Functions);
                }
            }

            return dao;
        }

        // detect all workflow connections WC
        // wc in WC entry (RECURSIVE)
        //    w <- find child workflow 
        //    c <- for each connection
        //        wc.effects <- add all postconditions 
        //        if connection is workflow
        //        wc.effects <- (RECURSIVE)
        // 

        

        private static List<WorkflowConnection> ExtractWorkflowConnections(InstitutionDao dao, WorkflowDao workflow)
        {
            var workflowConnections = new List<WorkflowConnection>();

            foreach (var connection in workflow.Connections.Where(w => !string.IsNullOrEmpty(w.ActionId)))
            {
                var action = workflow.Actions.FirstOrDefault(w => w.Id == connection.ActionId);
                if (action == null && dao.Globals != null && dao.Globals.Actions != null)
                {
                    action = dao.Globals.Actions.FirstOrDefault(w => w.Id == connection.ActionId);
                }

                if (action == null)
                {
                    throw new InstitutionException("Action does not exist: " + connection.ActionId);
                }

                var waction = action as ActionJoinWorkflowDao;
                if (waction != null)
                {
                    workflowConnections.Add(new WorkflowConnection(connection, dao.Workflows.First(w => w.Id == waction.WorkflowId)));
                }
            }

            return workflowConnections;
        }

        private List<AccessConditionDao> ExtractEffects(InstitutionDao dao, WorkflowConnection wc)
        {
            var effects = new List<AccessConditionDao>();

            if (wc.Workflow.Connections == null) return effects; // no connection in workflow

            foreach (var connection in wc.Workflow.Connections)
            {
                var conn = connection;
                
                // possibly import the connection

                if (!string.IsNullOrEmpty(connection.Import))
                {
                    conn = dao.Globals.Connections.First(w => w.Id == connection.Import);
                }

                // add postconditions 

                if (conn.Postconditions != null)
                {
                    effects.AddRange(conn.Postconditions);
                }

                // find all workflow connections

                foreach (var wconn in ExtractWorkflowConnections(dao, wc.Workflow))
                {
                    effects.AddRange(ExtractEffects(dao, wconn));
                }
            }

            // assign these effects to the current connection

            wc.Connection.GeneratedNestedEffects = effects.ToArray();

            return effects;
        } 

        private void CheckAndCombine(ParametricEntityDao fromItem, ParametricEntityDao toItem)
        {
            if (toItem != null && toItem.Name != fromItem.Name)
            {
                throw new NotImplementedException(
                    "It is not possible to import roles/organisations with same ids and different names");
            }

            // merge parameters
            if (toItem != null)
            {
                if (fromItem.Properties == null) fromItem.Properties = new List<ParameterDao>();
                
                    if (toItem.Properties == null) toItem.Properties = new List<ParameterDao>();

                    foreach (var param in fromItem.Properties)
                    {
                        if (!toItem.Properties.Exists(w => w.Name == param.Name))
                        {
                            toItem.Properties.Add(param);
                        }

                    }
                
            }
        }

        private void CheckByName(ParametricEntityDao fromItem, ParametricEntityDao toItem)
        {
            if (toItem.Id != fromItem.Id)
            {
                throw new NotImplementedException("It is not possible to import roles with different ids");
            }
        }

        // TODO: figure this bitch out!

        private void Merge(List<RoleDao> from, List<RoleDao> to)
        {
            if (from == null) { return; }

            foreach (var fromItem in from)
            {
                // check by ide
                var toItem = to.FirstOrDefault(w => w.Id == fromItem.Id);
                CheckAndCombine(fromItem, toItem);

                toItem = to.FirstOrDefault(w => w.Name == fromItem.Name);
                if (toItem == null)
                {
                    to.Add(fromItem);
                }
                else
                {
                    CheckByName(fromItem, toItem);
                }
            }
        }

        private void Merge(List<OrganisationDao> from, List<OrganisationDao> to)
        {
            if (from == null) { return; }

            foreach (var fromItem in from)
            {
                // check by ide
                var toItem = to.FirstOrDefault(w => w.Id == fromItem.Id);
                CheckAndCombine(fromItem, toItem);

                toItem = to.FirstOrDefault(w => w.Name == fromItem.Name);
                if (toItem == null)
                {
                    to.Add(fromItem);
                }
                else
                {
                    CheckByName(fromItem, toItem);
                }
            }
        }

        private void Merge(List<ClassDao> from, List<ClassDao> to)
        {
            if (from == null) { return; }

            foreach (var fromItem in from)
            {
                // check by ide
                var toItem = to.FirstOrDefault(w => w.Id == fromItem.Id);
                CheckAndCombine(fromItem, toItem);

                toItem = to.FirstOrDefault(w => w.Name == fromItem.Name);
                if (toItem == null)
                {
                    to.Add(fromItem);
                }
                else
                {
                    CheckByName(fromItem, toItem);
                }
            }
        }

        public bool Save(InstitutionDao dao, string target)
        {
            throw new NotImplementedException();
        }

        // factory methods

        public AccessConditionDao[] LoadAccess(string source)
        {
            return this.Deserialise<AccessConditionDao[]>(source);
        }

        public string SaveAccess(AccessConditionDao[] dao)
        {

            var writer = new StringWriter();
            var serializer = new Serializer(SerializationOptions.Roundtrip);
            serializer.Serialize(writer, dao);

            return writer.ToString();

        }

//        public List<ActivityDao> LoadArcs(string source)
//        {
//            return this.Deserialise<List<ActivityDao>>(source);
//        }
//
//        public string SaveArcs(List<ActivityDao> arcs)
//        {
//            var writer = new StringWriter();
//            var serializer = new Serializer(SerializationOptions.Roundtrip);
//            serializer.Serialize(writer, arcs);
//            return writer.ToString();
//        }


        T Deserialise<T>(string source)
        {
            var input = new StringReader(source);
            var deserializer = new Deserializer();

            deserializer.RegisterTagMapping("!workflow", typeof(ActionJoinWorkflowDao));
            deserializer.RegisterTagMapping("!action", typeof(ActionMessageDao)); 
            deserializer.RegisterTagMapping("!startWorkflow", typeof(ActionStartWorkflowDao));
            deserializer.RegisterTagMapping("!startAgent", typeof(ActionStartAgentDao));
            deserializer.RegisterTagMapping("!timeout", typeof(ActionTimeoutDao));

            deserializer.RegisterTagMapping("!binaryDecision", typeof(TransitionBinaryDecisionDao));
            deserializer.RegisterTagMapping("!split", typeof(TransitionSplitDao));
            deserializer.RegisterTagMapping("!join", typeof(TransitionJoinDao));
            

            return deserializer.Deserialize<T>(input);
        }
    }
}
