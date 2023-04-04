const toJson = (item) => {
    return {
        id : item.id,
        name : item.name ,
        //loaiDanhMuc:item.loaiDanhMuc,
        color:item.color
    }
}
const fromJson = (item) => {
    return {
        id : item.id,
        name : item.name ,
        //loaiDanhMuc:item.loaiDanhMuc,
        color:item.color
    }
}

const baseJson = () => {
    return {
        id : null,
        name : null ,
        color: null
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

export const loaiDanhMucModel = {
    toJson, fromJson, baseJson, toListModel
}
