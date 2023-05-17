import { expect, Locator, Page } from '@playwright/test';

export class RegisterObjectModel {
    readonly name = 'Krzysztof';
    readonly surname = 'Kowalski';
    readonly nickname;
    readonly email;
    readonly wrongEmail = 'abdc123';
    readonly password = '123!@#asdASD';
    readonly wrongPassword = 'Admin123';

    readonly page: Page;

    readonly loginLink: Locator;
    readonly logoutLink: Locator;
    readonly registerLink: Locator;
    readonly accountLink: Locator;

    readonly nameInput: Locator;
    readonly surnameInput: Locator;
    readonly nicknameInput: Locator;
    readonly emailInput: Locator;
    readonly passwordInput: Locator;
    readonly confirmPasswordInput: Locator;
    readonly submitButton: Locator;


    constructor(page: Page) {
        this.page = page;

        this.loginLink = page.locator('#login-button');
        this.logoutLink = page.locator('#logout-button');
        this.registerLink = page.locator('#register-button');
        this.accountLink = page.locator('#account-button');

        this.nameInput = page.locator('#name-input');
        this.surnameInput = page.locator('#surname-input');
        this.nicknameInput = page.locator('#nickname-input');
        this.emailInput = page.locator('#email-input');
        this.passwordInput = page.locator('#password-input');
        this.confirmPasswordInput = page.locator('#confirmPassword-input');
        this.submitButton = page.locator('p-button[type="submit"]');

        this.email = this.getRandomEmail();
        this.nickname = this.getRandomNickname();
    }

    private getRandomEmail() {
        return Math.random().toString(36).substring(2, 15) + '@test.com';
    }

    private getRandomNickname() {
        return Math.random().toString(36).substring(2, 15) + 'name';
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

    async loginFail() {
        await this.loginLink.click();
        await this.emailInput.fill(this.email);
        await this.passwordInput.fill(this.wrongPassword);
        await this.submitButton.click();
    }

    async expectLoginFail() {
        await expect(this.page).toHaveURL('http://localhost:4200/login');
        const token = await this.page.evaluate(() => localStorage.getItem('token'));
        expect(token).toBeNull();
    }

    async registerFail() {
        await this.registerLink.click();
        await this.nameInput.fill(this.name);
        await this.surnameInput.fill(this.surname);
        await this.emailInput.fill(this.wrongEmail);
        await this.passwordInput.fill(this.password);
        await this.confirmPasswordInput.fill(this.password);
        await this.submitButton.click();
    }

    async expectRegisterFail() {
        await expect(this.page).toHaveURL('http://localhost:4200/register');
    }

    async register() {
        await this.registerLink.click();
        await this.nameInput.fill(this.name);
        await this.surnameInput.fill(this.surname);
        await this.nicknameInput.fill(this.nickname);
        await this.emailInput.fill(this.email);
        await this.passwordInput.fill(this.password);
        await this.confirmPasswordInput.fill(this.password);
        await this.submitButton.click();
    }

    async expectRegisterSuccess() {
        await expect(this.page).toHaveURL('http://localhost:4200/login');
    }

    async checkUserData() {
        await this.accountLink.click();
        await this.page.waitForTimeout(5000);

        await expect(this.page).toHaveURL('http://localhost:4200/user');
        await expect(this.nameInput.inputValue()).resolves.toBe(this.name);
        await expect(this.surnameInput.inputValue()).resolves.toBe(this.surname);
        await expect(this.nicknameInput.inputValue()).resolves.toBe(this.nickname);
        await expect(this.emailInput.inputValue()).resolves.toBe(this.email);
    }

    async logout() {
        await this.logoutLink.click();
    }

    async expectLogoutSuccess() {
        const token = await this.page.evaluate(() => localStorage.getItem('token'));
        expect(token).toBeNull();
    }
}