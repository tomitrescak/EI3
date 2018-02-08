const {
  Sparky,
  FuseBox,
  JSONPlugin,
  CSSPlugin,
  CSSResourcePlugin,
  EnvPlugin,
  WebIndexPlugin,
  UglifyJSPlugin,
  QuantumPlugin
  } = require('fuse-box');
const StubPlugin = require('proxyrequire').FuseBoxStubPlugin(/\.tsx?/);
const JsxControlsPugin = require('jsx-controls-loader').fuseBoxPlugin;

// const uglify2 = {
//   compress: {
//     warnings: false,
//     screw_ie8: true,
//     conditionals: true,
//     unused: true,
//     comparisons: true,
//     sequences: true,
//     dead_code: true,
//     evaluate: true,
//     if_return: true,
//     join_vars: true
//   },
//   output: {
//     comments: false
//   },
//   sourceMap: false,
//   verbose: true,
//   exclude: /compiler\.js/i
// };


const productionFuse = FuseBox.init({
  homeDir: 'src',
  output: 'dist/$name',
  hash: true,
  tsConfig: 'tsconfig.prod.json',
  plugins: [
    EnvPlugin({ NODE_ENV: 'production' }),
    WebIndexPlugin({ template: 'src/index.html' }),
    //QuantumPlugin(),
    JsxControlsPugin,
    JSONPlugin(),
    //UglifyJSPlugin(),
    CSSPlugin({
      inject: false,
      group: 'bundle.css',
      outFile: `dist/bundle.css`
    }),
    // QuantumPlugin()
  ],
  shim: {
    crypto: {
      exports: '{ randomBytes: function(length) { return crypto.getRandomValues(new global.Uint8Array(length)) }}'
    }
  }
});

productionFuse
  .bundle('vendor.min.js')
  .target('browser')
  .instructions(' ~ index.ts'); // nothing has changed here

productionFuse
  .bundle('client.min.js')
  .target('browser')
  .instructions(' !> [index.ts]');

const express = require('express');
const historyAPIFallback = require('connect-history-api-fallback');
productionFuse.dev({ port: 3000 }, server => {
  const app = server.httpServer.app;
  app.use(express.static('dist'))
  app.use(historyAPIFallback());
});

productionFuse.run();

