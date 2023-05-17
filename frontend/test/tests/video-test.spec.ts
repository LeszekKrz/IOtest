import { test } from '@playwright/test';
import { VideoObjectModel } from '../test-object-models/video-object-model';

test('add-watch-video', async ({ page }) => {

  const videoObjectModel = new VideoObjectModel(page);
  await videoObjectModel.goToHome();

  await videoObjectModel.login();
  await videoObjectModel.expectLoginSuccess();

  await videoObjectModel.addVideo();
  await videoObjectModel.expectAddVideoSuccess();

  await videoObjectModel.logout();
  await videoObjectModel.expectLogoutSuccess();
});
