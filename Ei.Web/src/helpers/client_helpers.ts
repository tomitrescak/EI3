import { WorkHistory } from "../modules/history/history";
import { toast, ToastOptions } from "react-semantic-toasts";
import swal from "sweetalert2/dist/sweetalert2";
import type { SweetAlertIcon } from "sweetalert2";
import type { History } from "history";
import { useLocation } from "react-router-dom";
import { useMemo } from "react";

export function useQuery<T>(): T {
  const { search } = useLocation();

  return useMemo(() => {
    var params = new URLSearchParams(search);
    return Array.from(params.entries()).reduce((p, n) => {
      p[n[0]] = n[1];
      return p;
    }, {}) as any;
  }, [search]);
}

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
  alertDialog(name: string, text?: string, type: SweetAlertIcon = "error") {
    swal.fire(name, text, type);
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
    type: SweetAlertIcon = "warning"
  ) {
    const result = await swal.fire({
      title: name,
      text: text,
      icon: type,
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

    return swal.fire({
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
  ) {
    return swal.fire({
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
    return swal.fire({
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
  push(_route: string) {},
  replace(_route: string) {},
};
