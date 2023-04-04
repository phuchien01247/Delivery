const toJson = (item) => {
    return {
        id: item.id,
        userName: item.userName,
        firstName: item.firstName,
        lastName: item.lastName,
        fullName: item.fullName,
        phoneNumber: item.phoneNumber,
        email: item.email,
        note: item.note,
        avatar: item.avatar,
        role: item.role
        /* 
        donVi: item.donVi,permissions: item.permissions,
        menu: item.menu,
        kySo: item.kySo,
        chucVu: item.chucVu,
        userNameKySo: item.userNameKySo,
        passwordKySo: item.passwordKySo */
    }
}

const fromJson = (item) => {
    return {
        id: item.id,
        userName: item.userName,
        firstName: item.firstName,
        lastName: item.lastName,
        fullName: item.fullName,
        phoneNumber: item.phoneNumber,
        email: item.email,
        note: item.note,
        avatar: item.avatar,  
        role: item.role
        /* donVi: item.donVi,
        permissions: item.permissions,
        menu: item.menu,
        kySo: item.kySo,
        chucVu: item.chucVu,
        userNameKySo: item.userNameKySo,
        passwordKySo: item.passwordKySo */
    }
}

const baseJson = () => {
    return {
        id: null,
        userName: null,
        firstName: null,
        lastName: null,
        fullName: null,
        phoneNumber: null,
        email: null,
        role: null,
        note: null,
        avatar: null
        
        /*donVi: {id: null, ten: null}, 
        chucVu: null,
        kySo: null,
        userNameKySo: null,
        passwordKySo: null */
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
export const userModel = {
    toJson, fromJson, baseJson, toListModel
}
