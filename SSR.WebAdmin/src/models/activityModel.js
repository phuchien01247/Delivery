const toJson = (item) => {
    return {
        id : item.id,
        action : item.action ,
        content:item.content,
        createdBy:item.createdby,
        createdAt:item.createdAt,
        label:item.label,
        color:item.color,
        step:item.step,
        icon:item.icon,
    }
}
const fromJson = (item) => {
    return {
        id : item.id,
        action : item.action ,
        content:item.content,
        createdBy:item.createdby,
        createdAt:item.createdAt,
        label:item.label,
        color:item.color,
        step:item.step,
        icon:item.icon,
    }
}

const baseJson = () => {
    return {
        id : null,
        action : null ,
        content: null,
        createdBy:null,
        createdAt:null,
        label:null,
        color:null,
        step:null,
        icon:null
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

export const activityModel = {
    toJson, fromJson, baseJson, toListModel
}
