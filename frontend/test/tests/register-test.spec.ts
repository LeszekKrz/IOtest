import { test } from '@playwright/test';
import { RegisterObjectModel } from '../test-object-models/register-object-model';

test('Register', async ({ page }) => {

  const registerObjectModel = new RegisterObjectModel(page);
  await registerObjectModel.goToHome();

  await registerObjectModel.login();
  await registerObjectModel.expectLoginFail();

  await registerObjectModel.registerFail();
  await registerObjectModel.expectRegisterFail();

  await registerObjectModel.register();
  await registerObjectModel.expectRegisterSuccess();

  await registerObjectModel.login();
  await registerObjectModel.expectLoginSuccess();

  await registerObjectModel.checkUserData();

  await registerObjectModel.logout();
  await registerObjectModel.expectLogoutSuccess();
});
