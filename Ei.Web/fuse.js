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


function initServer(startServer, mode) {
  const serverFuse = FuseBox.init({
    homeDir: 'src',
    output: 'public/$name.js',
    sourceMaps: { project: false, vendor: false },
    plugins: [
      EnvPlugin({ NODE_ENV: mode }),
    ],
    tsConfig: startServer ? 'tsconfig.json' : 'tsconfig.server.json',
  });

  let server = serverFuse
    .bundle('server')
    .sourceMaps(false)
    .instructions(' > [server/index.ts]');
  // Execute process right after bundling is completed
  // launch and restart express

  if (startServer) {
    server
      .watch('server/**') // watch only server related code.. bugs up atm
      .cache(startServer);
  }

  if (startServer) {
    console.log('COMPLETED ...');
    server.completed(proc => proc.start());
  }
  serverFuse.run();
}

module.exports = {
  initServer
}

