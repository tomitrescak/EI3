const {
  Sparky,
  FuseBox,
  JSONPlugin,
  CSSPlugin,
  CSSResourcePlugin,
  EnvPlugin,
  WebIndexPlugin,
  } = require('fuse-box');

const JsxControlsPugin = require('jsx-controls-loader').fuseBoxPlugin;

function initFuse() {
  const defaultFuse = FuseBox.init({
    homeDir: 'src',
    output: 'public/$name.js',
    plugins: [
      WebIndexPlugin({ template: 'src/index.html', target: 'index.html' }),
      JsxControlsPugin,
      JSONPlugin(),
      CSSPlugin({
        group: 'bundle.css',
        outFile: `public/bundle.css`,
        inject: false
      })
    ],
    shim: {
      crypto: {
        exports: '{ randomBytes: () => crypto.getRandomValues(new global.Uint16Array(1))[0] }'
      }
    }
  });

  const historyAPIFallback = require('connect-history-api-fallback');
  defaultFuse.dev({ port: 3000 }, server => {
    const app = server.httpServer.app;
    app.use(historyAPIFallback());
  });

  defaultFuse
    .bundle('vendor')
    .hmr()
    //.watch()
    .target('browser')
    .instructions(' ~ index.ts'); // nothing has changed here

  defaultFuse
    .bundle('client')
    .watch() // watch only client related code
    .cache(true)
    .hmr()
    .target('browser@es6')
    .sourceMaps(true)
    .instructions(' !> [index.ts]');

  return defaultFuse;
}


const fuse = initFuse();
fuse.run();




