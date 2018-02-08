let a: any;

export const Ui = {
  alerter(v?: any) { 
    if (v) {
      a = v;
    }
    return a;
  },
  alert(text: string, _options?: Object) {
    if (!this.alerter) {
      return;
    }
    this.alerter().success(text); // .success(text, options);
  },
  alertError(text: string, _options?: Object) {
    if (!this.alerter) {
      return;
    }
    this.alerter().error(text); // sAlert.error(text, options);
  },
  alertDialog(name: string, text?: string, type = 'error') {
    const swal = require('sweetalert2');
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
          values: [current]
        });
      }
      return previous;
    }, []);
  },
  async confirmDialogAsync(
    text = `Do you want to delete this record? This action cannot be undone.`,
    name = `Are you sure?`,
    confirmButtonText = `Delete`,
    type = 'warning'
  ) {
    const swal = require('sweetalert2');
    try {
      await swal({
        name: name,
        text: text,
        type: type,
        showCancelButton: true,
        cancelButtonColor: '#d33',
        confirmButtonText: confirmButtonText
      });
      return true;
    } catch (ex) {
      /**/
    }
    return false;
  },
  inputValidator: (validate: (val: string) => any) => {
    return function(value: string) {
      return new Promise(function(resolve) {
        if (validate(value)) {
          resolve(null);
        } else {
          resolve(`Input is empty or has incorrect format!`);
        }
      });
    };
  },
  asyncPrompt(prompt: string, placeholder = '', validate = (val: string) => val !== '') {
    // let title = mf(prompt);
    const swal = require('sweetalert2');
    return swal({
      title: prompt,
      input: 'text',
      inputPlaceholder: placeholder,
      showCancelButton: false,
      allowEscapeKey: false,
      allowOutsideClick: false,
      inputValidator: this.inputValidator(validate)
    });
  },
  promptText(
    prompt: string,
    placeholder = '',
    validate = (val: string) => val !== ''
  ): Promise<{ value: string }> {
    const swal = require('sweetalert2');
    return swal({
      title: prompt,
      input: 'text',
      inputPlaceholder: placeholder,
      showCancelButton: true,
      inputValidator: this.inputValidator(validate)
    });
  },
  promptOptions(
    prompt: string,
    placeholder: string,
    options: { [idx: string]: string },
    validate = (val: string) => val
  ) {
    const swal = require('sweetalert2');
    return swal({
      title: prompt,
      input: 'select',
      inputOptions: options,
      inputPlaceholder: placeholder,
      showCancelButton: true,
      inputValidator: this.inputValidator(validate as any)
    });
  },
}

export const Router = {
  router: '',
  go(_route: string) {
    // RouterUtils.router.transitionTo(route);
    // history.push(route);
  }
};