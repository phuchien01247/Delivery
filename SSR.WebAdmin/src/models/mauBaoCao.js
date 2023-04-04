import moment from "moment";
const toJson = (item) => {
    return {
        id: item.id,
        ten: item.ten,
        thuTu: item.thuTu,
        dangSuDung:item.dangSuDung,
        ghiChu:item.ghiChu
    }
}
const fromJson = (item) => {
    return {
        id: item.id,
        ten: item.ten,
        thuTu: item.thuTu,
        dangSuDung:item.dangSuDung,
        ghiChu:item.ghiChu
    }
}

const baseJson = () => {
    return {
        id: null,
        ten: null,
        thuTu: 0,
        dangSuDung:true,
        ghiChu:null
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

export const mauBaoCaoModel = {
    toJson, fromJson, baseJson, toListModel,
}
