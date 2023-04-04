
const baseJson = () => {
    return {
        id: null,
        content: null,
        parentId: null,
        issueId: null,
        files: null,
        isDeleted: false
    }
}
const toJson = (item) => {
  return {
      id: item.id,
      content: item.content,
      parentId: item.parentId,
      issueId: item.issueId,
      files: item.files,
      isDeleted: item.isDeleted,
      createdBy: item.createdBy,
      createdAt: item.createdAt
  }
}
const fromJson = (item) => {
  return {
    id: item.id,
    content: item.content,
    parentId: item.parentId,
    issueId: item.issueId,
    files: item.files,
    isDeleted: item.isDeleted,
    createdBy: item.createdBy,
    createdAt: item.createdAt  
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

export const commentModel = {
  baseJson, toListGroup, fromJson, toJson
}
