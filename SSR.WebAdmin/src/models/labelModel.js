const toJson = (item) => {
    return {
        id : item.id,
        name : item.name ,
        IdProject:item.IdProject,
        color:item.color,
        parentId: item.parentId,
        knowledge: item.knowledge,
        isglobal: item.isglobal,
    }
}
const fromJson = (item) => {
    return {
        id : item.id,
        name : item.name ,
        IdProject:item.IdProject,
        color:item.color,
        parentId: item.parentId,
        knowledge: item.knowledge,
        isglobal: true,
    }
}

const baseJson = () => {
    return {
        id : null,
        name : null ,
        color: null,
        parentId: null,
        knowledge: null,
        isglobal: true,
        IdProject: null,
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

export const labelModel = {
    toJson, fromJson, baseJson, toListModel
}
