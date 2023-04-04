const toJson = (item) => {
    return {
        idLB: item.idLB,
        nameLB: item.nameLB,
        values: item.values,
        sort: item.sort      
    }
}

const fromJson = (item) => {
    return {
        idLB: item.idLB,
        nameLB: item.nameLB,
        values: item.values,
        sort: item.sort
    }
}

const baseJson = () => {
    return {
        idLB: null,
        nameLB: null,
        values: null,
        sort: 0
    }
}

const toListModel = (items) =>{
    if(items.length > 0){
        let data = [];
        items.map((value, index) =>{
            data.push(fromJson(value));
        })
        return data??[];
    }
    return [];
}
export const thongkeModel = {
    toJson, fromJson, baseJson, toListModel
}
