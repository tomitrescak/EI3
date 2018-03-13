String.prototype.toUrlName = function(this: string) {
  let result = this.indexOf('.') > 0 ? this.substring(0, this.lastIndexOf('.')) : this;
  let extension = this.indexOf('.') > 0 ? this.substring(this.lastIndexOf('.')) : '';
  result = result.replace(/\:/g, '');
  result = result.replace(/ - /g, '-');
  result = result.replace(/\W/g, '-');
  do {
    result = result.replace(/--/g, '-');
  } while (result.indexOf('--') >= 0);
  return (result + extension).toLowerCase();
};

String.prototype.toId = function(this: string) {
  
  let result = this.replace(/\:/g, '');
  result = result.replace(/ - /g, '');
  result = result.replace(/\W/g, ' ');
  do {
    result = result.replace(/  /g, ' ');
  } while (result.indexOf('  ') >= 0);

  let parts = result.split(' ');
  result = parts.map(w => w[0].toUpperCase() + w.substring(1)).join('');

  return result;
};

String.prototype.hashCode = function() {
  let hash = 0;

  if (this.length === 0) {
    return hash;
  }
  let chr: number;
  for (let i = 0; i < this.length; i++) {
    chr = this.charCodeAt(i);
    // tslint:disable-next-line:no-bitwise
    hash = (hash << 5) - hash + chr;
    // tslint:disable-next-line:no-bitwise
    hash |= 0; // Convert to 32bit integer
  }
  return hash;
};
