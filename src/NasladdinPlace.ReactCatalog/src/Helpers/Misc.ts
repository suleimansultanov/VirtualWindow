import getRandomValues from 'get-random-values';

export class MiscHelpers {

    static isClient = () => {
        return typeof document !== 'undefined' && document.createElement;
    }

    static generateGuid = () =>
        ((1e7).toString() + -1e3 + -4e3 + -8e3 + -1e11)
        .replace(/[018]/g, c => (Number(c) ^ getRandomValues(new Uint8Array(1))[0] & 15 >> Number(c) / 4).toString(16))

    static range = (length: number, start?: number) => Array.from(new Array(length), (_, i) => i + (start || 0));
}