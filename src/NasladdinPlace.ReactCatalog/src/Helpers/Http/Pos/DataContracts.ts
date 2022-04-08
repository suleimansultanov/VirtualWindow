
export type Pos = {
    id: number,
    name: string,
    location?: string,
    temperature: number,
    restrictedAccess: boolean,
};

export type PosGood = {
    id: number,
    maker: string,
    name: string,
    weight: number,
    composition?: string,
    description?: string,
    imagePath: string,
    publishingStatus: 0 | 1 | 2
    currency: string | null,
    price: number,
    count: number,
    nutrients?: {
        calories: number,
        proteins: number,
        fats: number,
        carbohydrates: number,
    },
};

export type PosContent = Array<{
    category: {
        id: number,
        name: string,
    },

    goods: Array<PosGood>,
}>;

export type PosListResponse = {
    lastVisited: Pos,
    items: Array<Pos>,
};
