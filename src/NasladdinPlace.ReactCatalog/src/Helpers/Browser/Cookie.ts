
export class CookieHelpers {

    public static getAllCookies = () => document.cookie.split(';').map(c => {
        const [name, value] = c.split('=').map(c => c.trim());
        return { name: name, value: value };
    })

    public static getCookie = (name: string) => CookieHelpers.getAllCookies().find(r => r.name === name);

    public static deleteCookie = (name: string) => document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:01 GMT;`;

    public static setCookie = (name: string, val: string) => document.cookie = `${name}=${val}; path=/`;

}