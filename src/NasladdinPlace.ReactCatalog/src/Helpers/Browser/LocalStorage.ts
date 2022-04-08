import { MiscHelpers } from '../Misc';

type LocalStorageMap = {
    AuthToken: string
};

export class LocalStorageHelpers {

    public static getValue<T extends keyof LocalStorageMap>(key: T): LocalStorageMap[T] | null {

        if (!MiscHelpers.isClient()) {
            throw 'Never: client only code';
        }

        const valueAsString = window.localStorage.getItem(key);
        return valueAsString === null ? null
            : JSON.parse(valueAsString) as LocalStorageMap[T];
    }

    public static setValue <T extends keyof LocalStorageMap> (key: T, value: LocalStorageMap[T]) : void {

        if (!MiscHelpers.isClient()) {
            throw 'Never: client only code';
        }

        window.localStorage.setItem(key, JSON.stringify(value));
    }

    public static deleteValue <T extends keyof LocalStorageMap> (key: T) : void {

        if (!MiscHelpers.isClient()) {
            throw 'Never: client only code';
        }

        window.localStorage.removeItem(key);
    }
}