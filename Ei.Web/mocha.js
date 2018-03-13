const { setup, setupJsxControls } = require('wafl');

// setup environment variables
process.env.TS_NODE_FAST = true;
process.env.NODE_ENV="test";

// setup mocha
require('ts-node/register');
require('chai-match-snapshot/mocha').setupMocha();

// setup app
setup();
setupJsxControls();