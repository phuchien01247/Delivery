const toJson = (item) => {
    return {
        imageBase64: item.imageBase64,
        type: item.type
    }
}

const fromJson = (item) => {
    return {
        imageBase64: item.imageBase64,
        type: item.type
    }
}

const baseJson = () => {
    return {
        imageBase64: null,
        type: 0
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

export const signatureVMModel = {
    toJson, fromJson, baseJson, toListModel
}
