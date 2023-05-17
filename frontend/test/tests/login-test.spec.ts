import { test } from '@playwright/test';
import { LoginObjectModel } from '../test-object-models/login-object-model';

test('LoginFail-Login-Logout', async ({ page }) => {

  const loginObjectModel = new LoginObjectModel(page);
  await loginObjectModel.goToHome();

  await loginObjectModel.loginFail();
  await loginObjectModel.expectLoginFail();

  await loginObjectModel.login();
  await loginObjectModel.expectLoginSuccess();

  await loginObjectModel.logout();
  await loginObjectModel.expectLogoutSuccess();
});
