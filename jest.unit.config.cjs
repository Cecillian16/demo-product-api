const { pathsToModuleNameMapper } = require('ts-jest');

module.exports = {
  preset: 'ts-jest',
  testEnvironment: 'node',
  roots: ['<rootDir>/test/unit'],
  testMatch: ['**/*.spec.ts'],
  collectCoverage: true,
  coverageDirectory: 'coverage/unit',
  coverageReporters: ['text', 'lcov', 'html'],
  reporters: [
    'default',
    [ 'jest-junit', { outputDirectory: './coverage', outputName: 'junit-unit.xml' } ]
  ],
  globals: {
    'ts-jest': {
      tsconfig: 'test/tsconfig.json'
    }
  }
};
