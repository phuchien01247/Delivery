const baseJson = () => {
    return {
        id: null,
        name: null,
        createBy: null,
        members: null,
        description: null
    }
}
const toJson = (item) => {
    return {
        id: item.id,
        name: item.name,
        createBy: item.createBy,
        description: item.description,
        members: item.members
        
    }
}
const fromJson = (item) => {
    return {
        id: item.id,
        name: item.name,
        createBy: item.createBy,
        description: item.description,
        members: item.members
        
    }
}
const toListGroup = (items) =>{
    if(items.length > 0){
        let data = [];
        items.map((value, index) =>{
            data.push(fromJson(value));
        })
        return data??[];
    }
    return [];
}

export const groupModel = {
    baseJson, toListGroup, fromJson, toJson
}
