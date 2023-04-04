import moment from 'moment';
const toJson = (item) => {
    return {
        id: item.id,
        name: item.name,
    }
}

const fromJson = (item) => {
    return {
        id: item.id,
       
        name: item.name,
        permissions: item.permissions,
        isDeleted: item.isDeleted
    }
}

const baseJson = () => {
    return {
        id: null,
      
        name: null,
       
        permissions: null,
       
        isDeleted: false
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

export const roleModel = {
    toJson, fromJson, baseJson, toListModel
}
