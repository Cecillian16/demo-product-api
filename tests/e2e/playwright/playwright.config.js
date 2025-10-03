const { defineConfig } = require('@playwright/test');

module.exports = defineConfig({
  timeout: 30 * 1000,
  retries: 0,
  use: {
    baseURL: process.env.E2E_BASE_URL || 'http://localhost:5000',
    trace: 'off',
  },
  reporter: [
    ['list'],
    ['junit', { outputFile: '../../results/playwright-junit.xml' }],
    ['html', { outputFolder: '../../results/playwright-report', open: 'never' }]
  ],
  projects: [
    {
      name: 'chromium',
      use: { browserName: 'chromium' }
    }
  ]
});
