import { expect, Locator, Page } from '@playwright/test';

export class LoginObjectModel {
    readonly email = 'simple@test.com';
    readonly password = '123!@#asdASD';
    readonly wrongPassword = 'Admin123';

    readonly page: Page;
    readonly loginLink: Locator;
    readonly logoutLink: Locator;
    readonly emailInput: Locator;
    readonly passwordInput: Locator;
    readonly submitButton: Locator;

    constructor(page: Page) {
        this.page = page;
        this.emailInput = page.locator('#email-input');
        this.passwordInput = page.locator('#password-input');
        this.submitButton = page.locator('p-button[type="submit"]');
        this.loginLink = page.locator('#login-button');
        this.logoutLink = page.locator('#logout-button');
    }

    async goToHome() {
        await this.page.goto('http://localhost:4200');
    }
    
    async loginFail() {
        await this.loginLink.click();
        await this.emailInput.fill(this.email);
        await this.passwordInput.fill(this.wrongPassword);
        await this.submitButton.click();
    }

    async login() {
        await this.loginLink.click();
        await this.emailInput.fill(this.email);
        await this.passwordInput.fill(this.password);
        await this.submitButton.click();
    }

    async logout() {
        await this.logoutLink.click();
    }

    async expectLoginSuccess() {
        await expect(this.page).toHaveURL('http://localhost:4200');
        const token = await this.page.evaluate(() => localStorage.getItem('token'));
        expect(token != null).toBeTruthy();
    }

    async expectLoginFail() {
        await expect(this.page).toHaveURL('http://localhost:4200/login');
        const token = await this.page.evaluate(() => localStorage.getItem('token'));
        expect(token).toBeNull();
    }

    async expectLogoutSuccess() {
        const token = await this.page.evaluate(() => localStorage.getItem('token'));
        expect(token).toBeNull();
    }
}