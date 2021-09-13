import * as SRD from "@projectstorm/react-diagrams";

import {
  FieldCollection,
  FieldDefinition,
  FieldMap,
  FieldModel,
  FormStateListener,
} from "semantic-ui-mobx";

export class FormNodeStore extends SRD.NodeModel {
  fields: FieldMap;
  fieldDefinitions: FieldDefinition[];
  formListeners: FormStateListener[] = [];
  validationMessage: string;
  __isChecked: boolean;

  constructor(id: string) {
    super({
      id,
      type: "default",
    });

    if (this.fieldDefinitions && this.fieldDefinitions.length) {
      this.fields = {};
      for (let field of this.fieldDefinitions) {
        this.fields[field.key] = new FieldModel(
          this,
          field.key,
          field.validators
        );
      }
    }
  }

  getPart(origin: any, key: string) {
    if (this.fields[key]) {
      return this.fields[key];
    }
    return origin[key];
  }

  checkField(listener: FormStateListener, field: any) {
    if (field instanceof FormNodeStore) {
      field.addFormListener(listener);
    } else if (field instanceof FieldModel) {
      field.listen(this.formListeners);
    } else if (field instanceof FieldCollection) {
      field.listen(this.formListeners);
    }
  }

  addFormListener(listener: FormStateListener) {
    if (this.__isChecked) {
      return;
    }
    this.__isChecked = true;

    this.formListeners.push(listener);

    // notify all fields
    for (let fieldName of Object.getOwnPropertyNames(this)) {
      let field = this.fields[fieldName]
        ? this.fields[fieldName]
        : this[fieldName];

      if (field != null && field.constructor.name === "ObservableArray") {
        if (field.___isChecked) {
          continue;
        }
        field.___isChecked = true;

        // add listener
        // field.observe(Ui.collectionObserver);

        for (let f of field) {
          this.checkField(listener, f);
        }
      } else {
        this.checkField(listener, field);
      }
    }
  }

  removeFormListener(listener: FormStateListener) {
    this.formListeners.splice(this.formListeners.indexOf(listener), 1);
  }

  changeField(key: string, value: any) {
    if (this.fields[key] === undefined) {
      throw new Error(`Store has no key "${key}"`);
    }
    (this as any)[key] = value;
  }

  validate(): string {
    let fields = (this as any).fields as { [index: string]: FieldModel<any> };
    this.validationMessage = "";
    for (let key of Object.getOwnPropertyNames(fields)) {
      let field = fields[key];
      let message = field.validate();
      if (message) {
        this.validationMessage += `Field '${field.key}': ${message}\n`;
      }
    }
    return this.validationMessage.trim();
  }

  isValid() {
    return this.validate() === "";
  }
}
