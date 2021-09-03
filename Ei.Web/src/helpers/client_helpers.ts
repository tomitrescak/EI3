import { WorkHistory } from "../modules/history/history";
import { toast, ToastOptions } from "react-semantic-toasts";
import swal, { SweetAlertType } from "sweetalert2";
import type { History } from "history";

export const Ui = {
  history: new WorkHistory(),
  // collectionObserver: (e: IArrayChange<any> | IArraySplice<any>) => {
  //   if (e.type === 'splice') {
  //     if (e.addedCount > 0) {
  //       Ui.history.addToCollection(e.object, e.added);
  //     } else if (e.removedCount > 0) {
  //       Ui.history.removeFromCollection(e.object, e.removed[0]);
  //     }
  //   } else if (e.type === 'update') {
  //     // Ui.history.addToCollection(this.parents, e.newValue);
  //   }
  // },
  alert(text: string, options?: ToastOptions) {
    toast({
      type: "success",
      title: text,
      ...options,
    }); // .success(text, options);
  },
  alertError(text: string, options?: ToastOptions) {
    toast({
      type: "error",
      title: text,
      ...options,
    });
  },
  alertDialog(name: string, text?: string, type: SweetAlertType = "error") {
    swal(name, text, type);
  },
  groupByArray<T>(xs: T[], key: string | Function): Array<Group<T>> {
    return xs.reduce(function (previous, current: Indexable<any>) {
      let v = key instanceof Function ? key(current) : current[key];
      let el = previous.find((r) => r && r.key === v);
      if (el) {
        el.values.push(current);
      } else {
        previous.push({
          key: v,
          values: [current],
        });
      }
      return previous;
    }, []);
  },
  async confirmDialogAsync(
    text = `Do you want to delete this record? This action cannot be undone.`,
    name = `Are you sure?`,
    confirmButtonText = `Delete`,
    type: SweetAlertType = "warning"
  ) {
    const result = await swal({
      title: name,
      text: text,
      type: type,
      showCancelButton: true,
      cancelButtonColor: "grey",
      confirmButtonText: confirmButtonText,
    });
    return result.value;
  },
  inputValidator: (validate: (val: string) => any) => {
    return function (value: string) {
      return new Promise(function (resolve) {
        if (validate(value)) {
          resolve(null);
        } else {
          resolve(`Input is empty or has incorrect format!`);
        }
      });
    };
  },
  asyncPrompt(
    prompt: string,
    placeholder = "",
    validate = (val: string) => val !== ""
  ) {
    // let title = mf(prompt);

    return swal({
      title: prompt,
      input: "text",
      inputPlaceholder: placeholder,
      showCancelButton: false,
      allowEscapeKey: false,
      allowOutsideClick: false,
      inputValidator: this.inputValidator(validate) as any,
    });
  },
  promptText(
    prompt: string,
    placeholder = "",
    validate = (val: string) => val !== ""
  ): Promise<{ value: string }> {
    return swal({
      title: prompt,
      input: "text",
      inputPlaceholder: placeholder,
      showCancelButton: true,
      inputValidator: this.inputValidator(validate) as any,
    });
  },
  promptOptions(
    prompt: string,
    placeholder: string,
    options: { [idx: string]: string },
    validate = (val: string) => val
  ) {
    return swal({
      title: prompt,
      input: "select",
      inputOptions: options,
      inputPlaceholder: placeholder,
      showCancelButton: true,
      inputValidator: this.inputValidator(validate as any) as any,
    });
  },
};

export const Router = {
  router: null as History,
  push(route: string) {},
  replace(route: string) {},
};
