import { computed, IObservableArray, makeObservable, observable } from "mobx";
import { field } from "semantic-ui-mobx";
import { DropdownItemProps } from "semantic-ui-react";
import createEngine, {
  DefaultLabelFactory,
  DefaultLinkFactory,
  DefaultNodeFactory,
  DiagramEngine,
  LinkLayerFactory,
  NodeLayerFactory,
} from "@projectstorm/react-diagrams";
import { FactoryBank } from "@projectstorm/react-canvas-core";

import { AppContext } from "../../config/context";

import { Ui } from "../../helpers/client_helpers";
import { EntityLinkFactory } from "../diagrams/model/entity/entity_link_factory";
import { EntityNodeFactory } from "../diagrams/model/entity/entity_node_factory";
import { Experiment } from "../experiments/experiment_model";
import { SocketClient } from "../ws/socket_client";
import { Authorisation, AuthorisationDao } from "./authorisation_model";
import { Entity } from "./entity_model";
import {
  HierarchicEntity,
  HierarchicEntityDao,
} from "./hierarchic_entity_model";
import {
  ParametricEntity,
  ParametricEntityDao,
} from "./parametric_entity_model";
import { Workflow, WorkflowDao } from "./workflow_model";

const emptyWorkflows: WorkflowDao[] = [];
const emptyAuthorisation: AuthorisationDao[] = [];

export class Organisation extends HierarchicEntity {
  allowEditIcon = true;

  constructor(
    model: HierarchicEntityDao,
    route: string,
    parents: IObservableArray<HierarchicEntity>,
    ei: Ei
  ) {
    super(model, route, parents, ei, true);

    this.allowEditIcon = true;

    if (!this.Icon) {
      this.Icon = "🏠";
    }

    makeObservable(this);
  }
}

export class Role extends HierarchicEntity {
  route = "roles";

  constructor(
    model: HierarchicEntityDao,
    route: string,
    parents: IObservableArray<HierarchicEntity>,
    ei: Ei
  ) {
    super(model, route, parents, ei, true);

    this.allowEditIcon = true;

    if (!this.Icon) {
      this.Icon = "👮🏼‍";
    }
  }
}
export class Type extends HierarchicEntity {
  route = "types";
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
  static create(id: string, name: string, context: AppContext) {
    return new Ei(
      {
        Id: id,
        Name: name,
        Description: "",
        Expressions: null,
        Organisations: [{ Id: "default", Name: "Default" }],
        Roles: [{ Id: "default", Name: "Citizen" }],
        Types: [],
        Workflows: [
          {
            Id: "main",
            Name: "Main",
            Static: true,
            Stateless: true,
            States: [
              {
                Id: "start",
                Name: "Start",
                IsStart: true,
                IsEnd: true,
                IsOpen: false,
              },
            ],
          },
        ],
        Authorisation: [],
        MainWorkflow: "main",
        Properties: [],
      },
      context
    );
  }

  engine: DiagramEngine;

  @field MainWorkflow: string;
  Expressions: string;

  Organisations: IObservableArray<Organisation>;
  Roles: IObservableArray<Role>;
  Types: IObservableArray<HierarchicEntity>;
  Workflows: IObservableArray<Workflow>;
  Authorisation: IObservableArray<Authorisation>;
  Experiments: IObservableArray<Experiment>;

  constructor(model: EiDao, public context: AppContext) {
    super(model);

    this.context = context;
    this.engine = createEngine();

    let nodeFactories = this.engine.getNodeFactories();
    let linkFactories = this.engine.getLinkFactories();
    let layerFactories = this.engine.getLayerFactories();
    let labelFactories = this.engine.getLabelFactories();

    // layerFactories.registerFactory(new LinkLayerFactory());
    // layerFactories.registerFactory(new NodeLayerFactory());

    // nodeFactories.registerFactory(new DefaultNodeFactory());
    // linkFactories.registerFactory(new DefaultLinkFactory());
    // labelFactories.registerFactory(new DefaultLabelFactory());

    linkFactories.registerFactory(new EntityLinkFactory("default"));
    linkFactories.registerFactory(new EntityLinkFactory("link"));
    nodeFactories.registerFactory(new EntityNodeFactory());

    // this.engine.registerFactoryBank(factoryBank);

    //this.engine.registerNodeFactory(new WorkflowNodeFactory());

    this.engine.maxNumberPointsPerLink = 1;

    this.Experiments = observable([new Experiment()]);

    this.Expressions = model.Expressions;
    this.MainWorkflow = model.MainWorkflow;
    this.Organisations = this.initHierarchy(
      "organisations",
      model.Organisations,
      observable([]),
      Organisation
    );
    this.Roles = this.initHierarchy("roles", model.Roles, observable([]), Role);
    this.Types = this.initHierarchy("types", model.Types, observable([]), Type);
    this.Workflows = observable(
      (model.Workflows || emptyWorkflows).map(
        (r) => new Workflow(r, this, this.context)
      )
    );
    this.Authorisation = observable(
      (model.Authorisation || emptyAuthorisation).map(
        (r) => new Authorisation(r)
      )
    );

    this.addFormListener(() => Ui.history.step());

    makeObservable(this);
  }

  @computed
  get types(): DropdownItemProps[] {
    return ["int", "float", "string", "bool"]
      .concat(this.Types.map((t) => t.Name))
      .map((i) => ({ text: i, value: i }));
  }

  @computed
  get workflowOptions(): DropdownItemProps[] {
    return this.Workflows.map((w) => ({ text: w.Name, value: w.Id }));
  }

  @computed
  get organisationsOptions(): DropdownItemProps[] {
    return this.Organisations.map((w) => ({ text: w.Name, value: w.Id }));
  }

  get removableOrganisationsOptions(): DropdownItemProps[] {
    return [{ text: "None", value: "" }].concat(
      this.organisationsOptions as any
    );
  }

  @computed
  get roleOptions(): DropdownItemProps[] {
    return this.Roles.map((w) => ({ text: w.Name, value: w.Id }));
  }

  createUrl(route: string, entity: { Id: string; Name: string }) {
    return `/ei/${this.Name.toUrlName()}/${
      this.id
    }/${route}/${entity.Name.toUrlName()}/${entity.Id}`;
  }

  editorHeight(value: string) {
    let lines = (value || "").split("\n").length;
    let height = lines * 18 + 5;
    return height < 100 ? 100 : height;
  }

  roleName(id: string) {
    let entity = this.Roles.find((r) => r.Id === id);
    return entity ? entity.Name : "<Role Deleted>";
  }

  organisationName(id: string) {
    let entity = this.Organisations.find((r) => r.Id === id);
    return entity ? entity.Name : "<Organisation Deleted>";
  }

  compile(client: SocketClient) {
    client.send({
      query: "CompileInstitution",
      variables: [JSON.stringify(this.json)],
      receiver: (resultString: string) => {
        const result = JSON.parse(resultString);
        this.context.compiledCode = result.Code;
        this.context.errors.replace(result.Errors ? result.Errors : empty);
        this.context.compiling = false;

        if (result.Errors) {
          Ui.alertError("Compilation Failed");
        } else {
          Ui.alert("Compilation Successful");
        }
      },
    });
    this.context.compiling = true;
  }

  checkExists(array: Entity[], name: string, entity: Entity) {
    let m = array.find((a) => a.Id === entity.Id);
    if (m) {
      Ui.alertDialog(`${name} with this Id already exists: ${entity.Id}`);
      return true;
    }
    return false;
  }

  save = () => {
    const key = "ws." + this.Id;
    const json = this.json;
    localStorage.setItem(key, JSON.stringify(json, null));
  };

  createAuthorisation = (e: React.MouseEvent<HTMLElement>) => {
    e.stopPropagation();
    e.preventDefault();

    this.Authorisation.push(new Authorisation());
  };

  createOrganisation = (e: React.MouseEvent<HTMLElement>) => {
    e.stopPropagation();
    e.preventDefault();

    Ui.promptText("Name of the new organisation?").then((name) => {
      if (name.value) {
        let org = new Organisation(
          { Name: name.value, Id: name.value.toId() } as any,
          "organisations",
          this.Organisations,
          this
        );
        if (!this.checkExists(this.Organisations, "Organisation", org)) {
          this.Organisations.push(org);
          this.context.Router.push(org.url);

          Ui.history.step();
        }
      }
    });

    return false;
  };

  createRole = async (e: React.MouseEvent<HTMLElement>) => {
    e.stopPropagation();

    let name = await Ui.promptText("Name of the new role?");
    if (name.value) {
      let role = new Role(
        { Name: name.value, Id: name.value.toId() } as any,
        "roles",
        this.Roles,
        this
      );
      if (!this.checkExists(this.Roles, "Role", role)) {
        this.Roles.push(role);
        this.context.Router.push(role.url);

        Ui.history.step();
      }
    }
  };

  createType = async (e: React.MouseEvent<HTMLElement>) => {
    e.stopPropagation();

    let name = await Ui.promptText("Name of the new type?");
    if (name.value) {
      const type = new Type(
        { Name: name.value, Id: name.value.toId() } as any,
        "types",
        this.Types,
        this
      );
      if (!this.checkExists(this.Types, "Type", type)) {
        this.Types.push(type);
        this.context.Router.push(type.url);

        Ui.history.step();
      }
    }
  };

  createWorkflow = async () => {
    let name = await Ui.promptText("Name of the new workflow?");
    if (name.value) {
      let workflow = new Workflow(
        {
          Name: name.value,
          Id: name.value.toId(),
          Static: false,
          Stateless: false,
        } as any,
        this,
        this.context
      );
      if (!this.checkExists(this.Workflows, "Workflow", workflow)) {
        this.Workflows.push(workflow);
        this.context.Router.push(this.createUrl("workflows", workflow));

        Ui.history.step();
      }
    }
  };

  get json(): EiDao {
    return {
      ...super.json,
      MainWorkflow: this.MainWorkflow,
      Expressions: this.Expressions,
      Organisations: this.Organisations.map((o) => o.json),
      Roles: this.Roles.map((o) => o.json),
      Types: this.Types.map((o) => o.json),
      Workflows: this.Workflows.map((o) => o.json),
      Authorisation: this.Authorisation.map((o) => o.json),
    };
  }

  private initHierarchy(
    route: string,
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
        if (item.Parent == null || target.find((t) => t.Id === item.Parent)) {
          target.push(new ClassType(item, route, target, this));
          items.splice(items.indexOf(item), 1);
        } else {
          continue;
        }
      }
    }
    return target;
  }
}
