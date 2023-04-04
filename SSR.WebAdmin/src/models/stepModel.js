const toJson = (item) => {
    return {
        id: item.id,
        color: item.color,
        name: item.name,
        description: item.description,
        projectId:item.projectId,
    }
}

const fromJson = (item) => {
    return {
        id: item.id,
        color: item.color,
        name: item.name,
        description: item.description,
        projectId:item.projectId,

    }
}

const baseJson = () => {
    return {
        id: null,
        color: null,
        name: null,
        description: null,
        projectId:null,
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
export const stepModel = {
    toJson, fromJson, baseJson, toListModel
}
