import { autorun, computed, IObservableArray, observable } from 'mobx';
import { field } from 'semantic-ui-mobx';
import { DropdownItemProps } from 'semantic-ui-react';
import { DefaultNodeFactory, DiagramEngine } from 'storm-react-diagrams';

import { Ui } from '../../helpers/client_helpers';
import { EntityLinkFactory } from '../diagrams/model/entity/entity_link_factory';
import { EntityNodeFactory } from '../diagrams/model/entity/entity_node_factory';
import { SocketClient } from '../ws/socket_client';
import { Authorisation, AuthorisationDao } from './authorisation_model';
import { Entity } from './entity_model';
import { HierarchicEntity, HierarchicEntityDao } from './hierarchic_entity_model';
import { ParametricEntity, ParametricEntityDao } from './parametric_entity_model';
import { Workflow, WorkflowDao } from './workflow_model';

const emptyWorkflows: WorkflowDao[] = [];
const emptyAuthorisation: AuthorisationDao[] = [];

export class Organisation extends HierarchicEntity {
  allowEditIcon = true;

  constructor(model: HierarchicEntityDao, parents: IObservableArray<HierarchicEntity>, ei: Ei) {
    super(model, parents, ei, true);

    this.allowEditIcon = true;

    if (!this.Icon) {
      this.Icon = '🏠';
    }
  }

  select() {
    this.ei.store.viewStore.showOrganisation(this.Id, this.Name);
  }
}

export class Role extends HierarchicEntity {
  constructor(model: HierarchicEntityDao, parents: IObservableArray<HierarchicEntity>, ei: Ei) {
    super(model, parents, ei, true);

    this.allowEditIcon = true;

    if (!this.Icon) {
      this.Icon = '👮🏼‍';
    }
  }

  select() {
    this.ei.store.viewStore.showRole(this.Id, this.Name);
  }
}
export class Type extends HierarchicEntity {
  select() {
    this.ei.store.viewStore.showType(this.Id, this.Name);
  }
}

export interface EiDao extends ParametricEntityDao {
  Expressions: string;
  Organisations: HierarchicEntityDao[];
  Roles: HierarchicEntityDao[];
  Types: HierarchicEntityDao[];
  Workflows: WorkflowDao[];
  Authorisation: AuthorisationDao[];
  MainWorkflow: string;
}

const empty: string[] = [];

export class Ei extends ParametricEntity {
  static create(id: string, name: string, store: App.Store) {
    return new Ei(
      {
        Id: id,
        Name: name,
        Description: '',
        Expressions: null,
        Organisations: [{ Id: 'default', Name: 'Default' }],
        Roles: [{ Id: 'default', Name: 'Citizen' }],
        Types: [],
        Workflows: [
          {
            Id: 'main',
            Name: 'Main',
            Static: true,
            Stateless: true,
            States: [{ Id: 'start', Name: 'Start', IsStart: true, IsEnd: true, Open: false }]
          }
        ],
        Authorisation: [],
        MainWorkflow: 'main',
        Properties: []
      },
      store
    );
  }

  engine: DiagramEngine;
  store: App.Store;

  @field MainWorkflow: string;
  Expressions: string;

  Organisations: IObservableArray<Organisation>;
  Roles: IObservableArray<Role>;
  Types: IObservableArray<HierarchicEntity>;
  Workflows: IObservableArray<Workflow>;
  Authorisation: IObservableArray<Authorisation>;

  constructor(model: EiDao, store: App.Store) {
    super(model);

    this.store = store;
    this.engine = new DiagramEngine();
    this.engine.registerNodeFactory(new DefaultNodeFactory());
    this.engine.registerLinkFactory(new EntityLinkFactory('default'));
    this.engine.registerLinkFactory(new EntityLinkFactory('link'));
    this.engine.registerNodeFactory(new EntityNodeFactory());

    // this.engine.registerNodeFactory(new WorkflowNodeFactory());

    this.engine.maxNumberPointsPerLink = 1;

    this.Expressions = model.Expressions;
    this.MainWorkflow = model.MainWorkflow;
    this.Organisations = this.initHierarchy(model.Organisations, observable([]), Organisation);
    this.Roles = this.initHierarchy(model.Roles, observable([]), Role);
    this.Types = this.initHierarchy(model.Types, observable([]), Type);
    this.Workflows = observable(
      (model.Workflows || emptyWorkflows).map(r => new Workflow(r, this))
    );
    this.Authorisation = observable(
      (model.Authorisation || emptyAuthorisation).map(r => new Authorisation(r))
    );

    this.addFormListener(() => Ui.history.step());
  }

  @computed
  get types(): DropdownItemProps[] {
    return ['int', 'float', 'string', 'bool']
      .concat(this.Types.map(t => t.Name))
      .map(i => ({ text: i, value: i }));
  }

  @computed
  get workflowOptions(): DropdownItemProps[] {
    return this.Workflows.map(w => ({ text: w.Name, value: w.Id }));
  }

  @computed
  get organisationsOptions(): DropdownItemProps[] {
    return this.Organisations.map(w => ({ text: w.Name, value: w.Id }));
  }

  get removableOrganisationsOptions(): DropdownItemProps[] {
    return [{ text: 'None', value: '' }].concat(this.organisationsOptions as any);
  }

  @computed
  get roleOptions(): DropdownItemProps[] {
    return this.Roles.map(w => ({ text: w.Name, value: w.Id }));
  }

  editorHeight(value: string) {
    let lines = (value || '').split('\n').length;
    let height = (lines) * 18 + 5;
    return height < 100 ? 100 : height;
  }

  roleName(id: string) {
    let entity = this.Roles.find(r => r.Id === id);
    return entity ? entity.Name : '<Role Deleted>';
  }

  organisationName(id: string) {
    let entity = this.Organisations.find(r => r.Id === id);
    return entity ? entity.Name : '<Organisation Deleted>';
  }

  compile(client: SocketClient) {
    const observer = client.send('CompileInstitution', [JSON.stringify(this.json)]);
    this.store.compiling = true;
    autorun(() => {
      if (observer.loading) {
        // console.log('Compiling ...');
      } else {     
        let result = observer.data.CompileInstitution;

        this.store.compiledCode = result.Code;
        this.store.errors.replace(result.Errors ? result.Errors : empty);
        this.store.compiling = false;

        if (result.Errors) {
          Ui.alertError('Compilation Failed')
        } else {
          Ui.alert('Compilation Successful');
        }
        // console.log(JSON.stringify(observer.data));
      }
    });
  }

  checkExists(array: Entity[], name: string, entity: Entity) {
    let m = array.find(a => a.Id === entity.Id);
    if (m) {
      Ui.alertDialog(`${name} with this Id already exists: ${entity.Id}`);
      return true;
    }
    return false;
  }

  save = () => {
    const key = 'ws.' + this.Id;
    const json = this.json;
    localStorage.setItem(key, JSON.stringify(json, null));
  };

  createAuthorisation = (e: React.MouseEvent<HTMLElement>) => {
    e.stopPropagation();
    e.preventDefault();
  
    this.Authorisation.push(new Authorisation())
  }

  createOrganisation = (e: React.MouseEvent<HTMLElement>) => {
    e.stopPropagation();
    e.preventDefault();

    Ui.promptText('Name of the new organisation?').then(name => {
      if (name.value) {
        let org = new Organisation(
          { Name: name.value, Id: name.value.toId() } as any,
          this.Organisations,
          this
        );
        if (!this.checkExists(this.Organisations, 'Organisation', org)) {
          this.Organisations.push(org);
          this.store.viewStore.showOrganisation(org.Id, org.Name);

          Ui.history.step();
        }
      }
    });

    return false;
  };
  

  createRole = async (e: React.MouseEvent<HTMLElement>) => {
    e.stopPropagation();

    let name = await Ui.promptText('Name of the new role?');
    if (name.value) {
      let role = new Role(
        { Name: name.value, Id: name.value.toId() } as any,
        this.Roles,
        this
      );
      if (!this.checkExists(this.Roles, 'Role', role)) {
        this.Roles.push(role);
        this.store.viewStore.showRole(role.Id, role.Name);

        Ui.history.step();
      }
    }
  };

  createType = async (e: React.MouseEvent<HTMLElement>) => {
    e.stopPropagation();

    let name = await Ui.promptText('Name of the new type?');
    if (name.value) {
      const type = new Type(
        { Name: name.value, Id: name.value.toId() } as any,
        this.Types,
        this
      );
      if (!this.checkExists(this.Types, 'Type', type)) {
        this.Types.push(type);
        this.store.viewStore.showType(type.Id, type.Name);

        Ui.history.step();
      }
    }
  };

  createWorkflow = async () => {
    let name = await Ui.promptText('Name of the new workflow?');
    if (name.value) {
      let workflow = new Workflow(
        { Name: name.value, Id: name.value.toId(), Static: false, Stateless: false } as any,
        this
      );
      if (!this.checkExists(this.Workflows, 'Workflow', workflow)) {
        this.Workflows.push(workflow);
        this.store.viewStore.showWorkflow(workflow.Id, workflow.Name);

        Ui.history.step();
      }
    }
  };

  get json(): EiDao {
    return {
      ...super.json,
      MainWorkflow: this.MainWorkflow,
      Expressions: this.Expressions,
      Organisations: this.Organisations.map(o => o.json),
      Roles: this.Roles.map(o => o.json),
      Types: this.Types.map(o => o.json),
      Workflows: this.Workflows.map(o => o.json),
      Authorisation: this.Authorisation.map(o => o.json)
    };
  }

  private initHierarchy(
    entities: HierarchicEntityDao[],
    target: IObservableArray<HierarchicEntity>,
    ClassType: any
  ): IObservableArray<HierarchicEntity> {
    if (entities == null) {
      return target;
    }
    let items = [...entities];
    while (items.length > 0) {
      for (let item of items) {
        if (item.Parent == null || target.find(t => t.Id === item.Parent)) {
          target.push(new ClassType(item, target, this));
          items.splice(items.indexOf(item), 1);
        } else {
          continue;
        }
      }
    }
    return target;
  }
}