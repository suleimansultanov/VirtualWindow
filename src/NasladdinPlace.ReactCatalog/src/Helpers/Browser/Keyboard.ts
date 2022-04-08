import React from 'react';

export class KeyboardHelpers {

    static anchorClick(func: () => any): (e: React.MouseEvent<HTMLAnchorElement>) => void {
        return e => {
            e.preventDefault();
            func();
        };
    }

    static onControlKeys(handlers: { esc?: () => void, enter?: () => void }) {
        return (e: React.KeyboardEvent<HTMLTextAreaElement | HTMLInputElement>) => {
            const isEnter = e.keyCode === 13;
            if (isEnter && handlers.enter) {
                handlers.enter();
            }

            const isEsc = e.keyCode === 27;
            if (isEsc && handlers.esc) {
                handlers.esc();
            }
        };
    }

    static inputChanged(func: (value: string) => any): React.FormEventHandler<{}> {
        return e => {
            func((e.target as any as { value: string }).value);
        };
    }
}