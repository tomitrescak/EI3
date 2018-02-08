const {
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

let serverRunning = false;

const luisFuse = FuseBox.init({
  homeDir: 'src',
  emitHMRDependencies: true,
  output: 'public/$name.js',
  plugins: [
    JsxControlsPugin,
    EnvPlugin({ NODE_ENV: 'test', Luis: 'true' }),
    JSONPlugin(),
    [
      CSSResourcePlugin(),
      CSSPlugin({
        group: 'luis.css',
        outFile: `public/styles/luis.css`,
        inject: false
      })
    ],
    WebIndexPlugin({ template: 'src/client/luis.html', target: 'luis.html' })
  ],
  shim: {
    crypto: {
      exports: '{ randomBytes: () => crypto.getRandomValues(new global.Uint16Array(1))[0] }'
    },
    stream: {
      exports: '{ Writable: function() {}, Readable: function() {}, Transform: function() {} }'
    }
  }
});

luisFuse.dev({ port: 4445, httpServer: false });

luisFuse
  .bundle('luis-vendor')
  .hmr()
  .target('browser')
  .instructions(' ~ client/luis.ts'); // nothing has changed here

luisFuse
  .bundle('luis-client')
  .watch() // watch only client related code
  .hmr()
  .target('browser')
  .sourceMaps(true)
  .plugin([StubPlugin, JsxControlsPugin])
  .globals({
    proxyrequire: '*'
  })
  .completed(() => {
    if (!serverRunning) {
      serverFuse.run();
      serverRunning = true;
    }
  })
  .instructions(' !> [client/luis.ts] +proxyrequire'); // + **/**.json

// server

const serverFuse = FuseBox.init({
  homeDir: 'src',
  output: 'public/$name.js'
});
serverFuse
  .bundle('luis-server')
  .watch('server/**') // watch only server related code.. bugs up atm
  .instructions(' > [server/luis.ts]')
  .completed(proc => proc.start());

luisFuse.run();
