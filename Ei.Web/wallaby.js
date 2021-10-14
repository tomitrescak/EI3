// @ts-nocheck
var path = require("path");

module.exports = function (wallaby) {
  return {
    files: [
      "src/**/*.ts",
      "src/**/*.tsx",
      "!src/**/*.test.ts?(x)",
      "public/**/*.mustache",
    ],
    tests: ["src/**/*.test.ts?(x)"],
    env: {
      type: "node",
      NODE_ENV: "test",
      WALLABY: "1",
      ROOT_URL: "http://localhost:3000",
    },
    testFramework: "mocha",
    // compilers: {
    //   '**/*.ts?(x)': wallaby.compilers.babel({
    //     presets: ['next/babel'],
    //     plugins: [
    //       'jsx-control-statements',
    //       ['@babel/plugin-proposal-decorators', { legacy: true }],
    //       ['@babel/plugin-proposal-class-properties', { loose: true }]
    //     ]
    //   })
    // },
    delays: {
      run: 1000,
    },
    // workers: {
    //   initial: 1,
    //   regular: 1
    // },
    filesWithNoCoverageCalculated: ["src/**/*.stories.*", "src/**/stories/**"],
    reportUnhandledPromises: true,
    // setup: function (wallaby) {
    //   wallaby.delayStart();

    //   // wallaby.testFramework.grep('@server');
    //   // wallaby.testFramework.invert();

    //   require.extensions['.css'] = () => {
    //     /* */
    //   };

    //   if (!global.mongod) {
    //     global.mongod = { stop: () => Promise.resolve() };
    //   }

    //   console.log('Stopping daemon');
    //   global.mongod.stop().then(() => {
    //     console.log('Spinning new db');
    //     require('apollo-connector-mongodb/dist/testing')
    //       .getDb()
    //       .then(() => {
    //         console.log('Started new database instance!');
    //         wallaby.start();
    //       });
    //   });

    //   if (!global.document) {
    //     // eslint-disable-next-line @typescript-eslint/no-var-requires
    //     global.jsdom = require('jsdom-global')(undefined, {
    //       url: 'http://localhost',
    //       pretendToBeVisual: true
    //     });

    //     const { createSVGWindow } = require('svgdom');
    //     const window = createSVGWindow();
    //     const document = window.document;
    //     const { SVG, registerWindow } = require('@svgdotjs/svg.js');

    //     // register window and document
    //     registerWindow(window, document);
    //   }

    //   const mocha = wallaby.testFramework;
    //   mocha.suite.on('pre-require', function () {
    //     require(wallaby.projectCacheDir + '/src/mocha.config');

    //     const cleanup = require('@testing-library/react').cleanup;
    //     afterEach(cleanup);

    //     require(wallaby.projectCacheDir + '/src/compilers/java/lib/extendExpect');
    //   });

    //   // if (!delayed) {
    //   //   wallaby.start();
    //   // }
    // },
    // teardown: function (wallaby) {
    //   // console.log(global.db);
    //   // console.log('Database Teardown ..');
    //   // global.jsdom();
    // }
  };
};
