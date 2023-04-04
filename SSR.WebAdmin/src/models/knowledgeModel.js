const toJson = (item) => {
    return {
        id : item.id,
        name: item.name,
        summary: item.summary,
        content : item.content ,
    }
}
const fromJson = (item) => {
    return {
        id : item.id,
        name: item.name,
        summary: item.summary,
        content : item.content ,
    }
}

const baseJson = () => {
    return {
        id : null,
        name: null,
        summary: null,
        content : null ,
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

export const knowledgeModel = {
    toJson, fromJson, baseJson, toListModel
}
