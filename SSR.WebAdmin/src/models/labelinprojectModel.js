const toJson = (item) => {
    return {
        id : item.id,
        name : item.name ,
        idProject:item.idProject,
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
        idProject:item.idProject,
        color:item.color,
        parentId: item.parentId,
        knowledge: item.knowledge,
        isglobal: false,
    }
}

const baseJson = () => {
    return {
        id : null,
        name : null ,
        color: null,
        parentId: null,
        knowledge: null,
        isglobal: false,
        idProject: null,
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

export const labelinprojectModel = {
    toJson, fromJson, baseJson, toListModel
}
