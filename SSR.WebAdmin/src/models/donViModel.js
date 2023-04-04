import moment from "moment";
const toJson = (item) => {
    return {
        id : item.id,
        maDonVi : item.maDonVi,
        name : item.name ,
        thuTu : item.thuTu,
        parentId : item.parentId,
        capDV : item.capDV,
    }
}
const fromJson = (item) => {
    return {
        id : item.id,
        maDonVi : item.maDonVi,
        name : item.name ,
        thuTu : item.thuTu,
        parentId : item.parentId,
        capDV : item.capDV,
    }
}

const baseJson = () => {
    return {
        id : null,
        maDonVi : null,
        name : null,
        thuTu : 0,
        parentId : null,
        capDV : null,
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

export const donViModel = {
    toJson, fromJson, baseJson, toListModel
}
