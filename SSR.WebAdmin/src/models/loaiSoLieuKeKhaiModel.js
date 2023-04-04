const toJson = (item) => {
    return {
        id : item.id,
        ten : item.ten ,
        code:item.code,
        soLieuKeKhai:item.soLieuKeKhai,
        thuTu:item.thuTu
    }
}
const fromJson = (item) => {
    return {
        id : item.id,
        ten : item.ten ,
        thuTu:item.thuTu,
        code:item.code,
        soLieuKeKhai:item.soLieuKeKhai,
    }
}

const baseJson = () => {
    return {
        id : null,
        ten : null ,
        thuTu:0,
        code:null,
        soLieuKeKhai:null,
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

export const loaiSoLieuKeKhaiModel = {
    toJson, fromJson, baseJson, toListModel
}
