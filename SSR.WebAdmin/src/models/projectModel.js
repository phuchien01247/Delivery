
const baseJson = () => {
    return {
        id: null,
        name: null,
        nameNUni: null,
        description: null,
        slug: null,
        group: null,
        label: null,
        member: null,
        files: null,
        createdBy:null,
        createdAt:null,
        //uploadFiles: null,
        isDeleted: false
    }
}
const toJson = (item) => {
  return {
      id: item.id,
      name: item.name,
      nameNUni: item.nameNUni,
      description: item.description,
      slug: item.slug,
      member: item.member,
      group: item.group,
      label: item.label,
      files: item.files,
      createdBy: item.createdBy,
      createdAt: item.createdAt
      //uploadFiles: item.uploadFiles
  }
}
const fromJson = (item) => {
  return {
      id: item.id,
      name: item.name,
      nameNUni: item.nameNUni,
      description: item.description,
      slug: item.slug,
      member: item.member,
      group: item.group,
      label: item.label,
      files: item.files,
      createdBy: item.createdBy,
      createdAt:item.createdAt
      //uploadFiles: item.uploadFiles     
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

export const projectModel = {
  baseJson, toListGroup, fromJson, toJson
}
