const toJson = (item) => {
    return {
        id : item.id,
        title : item.title ,
        stepId:item.stepId,
        projectId:item.projectId,
        label: item.label,
        dueDate: item.dueDate,
        donvi: item.donvi,
        group: item.group,
        user: item.user,
        description: item.description,
        createdBy: item.createdBy,
        createdAt: item.createdAt
    }
}
const fromJson = (item) => {
    return {
        id : item.id,
        title : item.title ,
        stepId:item.stepId,
        projectId:item.projectId,
        label: item.label,
        dueDate: item.dueDate,
        donvi: item.donvi,
        group: item.group,
        user: item.user,
        description: item.description,
        createdBy: item.createdBy,
        createdAt: item.createdAt,
    }
}

const baseJson = () => {
    return {
        id: null,
        title : null ,
        stepId: "Open",
        projectId: null,
        label: null,
        dueDate: null,
        donvi: null,
        group: null,
        user: null,
        description: null,
        createdBy: null,
        createdAt: null
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

export const yeucauloiModel = {
    toJson, fromJson, baseJson, toListModel
}
