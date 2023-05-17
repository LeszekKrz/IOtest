import { expect, Locator, Page } from '@playwright/test';

export class VideoObjectModel {
    readonly email = 'creator@test.com';
    readonly password = '123!@#asdASD';
    readonly title = this.getRandomTitle();
    readonly description = 'description';
    readonly authorNickname = 'TestCreator';

    readonly page: Page;

    readonly loginLink: Locator;
    readonly addVideoLink: Locator;
    readonly logoutLink: Locator;

    readonly emailInput: Locator;
    readonly passwordInput: Locator;
    readonly submitButton: Locator;
    readonly videoTitleInput: Locator;
    readonly videoDescriptionInput: Locator;
    readonly thumbnailFileUpload: Locator;
    readonly videoFileUpload: Locator;
    readonly searchInput: Locator;
    readonly searchButton: Locator;


    constructor(page: Page) {
        this.page = page;
        
        this.loginLink = page.locator('#login-button');
        this.addVideoLink = page.locator('#add-video-button')
        this.logoutLink = page.locator('#logout-button');

        this.emailInput = page.locator('#email-input');
        this.passwordInput = page.locator('#password-input');
        this.submitButton = page.locator('p-button[type="submit"]');
        this.videoTitleInput = page.locator('#title-input');
        this.videoDescriptionInput = page.locator('#description-input');
        this.thumbnailFileUpload = page.locator('#thumbnail-file-upload');
        this.videoFileUpload = page.locator('#video-file-upload');
        this.searchInput = page.locator('#search-input');
        this.searchButton = page.locator('#search-button');
    }

    private getRandomTitle() {
        return Math.random().toString(36).substring(2, 15) + 'video';
    }

    async goToHome() {
        await this.page.goto('http://localhost:4200');
    }

    async login() {
        await this.loginLink.click();
        await this.emailInput.fill(this.email);
        await this.passwordInput.fill(this.password);
        await this.submitButton.click();
    }

    async expectLoginSuccess() {
        await expect(this.page).toHaveURL('http://localhost:4200');
        const token = await this.page.evaluate(() => localStorage.getItem('token'));
        expect(token != null).toBeTruthy();
    }

    async addVideo() {
        await this.addVideoLink.click();
        await this.videoTitleInput.fill(this.title);
        await this.videoDescriptionInput.fill(this.description);
        await this.page.setInputFiles('#thumbnail-file-upload input[type="file"]', 'test/test-data/thumbnail.png');
        await this.page.setInputFiles('#video-file-upload input[type="file"]', 'test/test-data/video.mp4');
        await this.page.waitForTimeout(3000);
        await this.page.locator('#submit-video-button').click();
    }

    async expectAddVideoSuccess() {
        await this.page.waitForTimeout(3000);
        await this.searchInput.fill(this.title);
        await this.searchButton.click();
        await this.page.waitForTimeout(3000);
        await this.page.click('.video:first-child');
        await this.page.waitForTimeout(3000);
        await expect(this.page.locator('#title').innerText()).resolves.toBe(this.title);
        await expect(this.page.locator('#author-nickname').innerText()).resolves.toBe(this.authorNickname);
    }

    async logout() {
        await this.logoutLink.click();
    }

    async expectLogoutSuccess() {
        const token = await this.page.evaluate(() => localStorage.getItem('token'));
        expect(token).toBeNull();
    }
}