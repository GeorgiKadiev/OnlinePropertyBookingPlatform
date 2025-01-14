import { configureStore } from "@reduxjs/toolkit";
import tokenReducer from "../utils/tokenSlice";
import roleReducer from "../utils/roleSlice";
import idReducer from "../utils/idSlice";
import filterReducer from "../utils/filterSlice";

const store = configureStore({
  reducer: {
    token: tokenReducer,
    role: roleReducer,
    id: idReducer,
    filter: filterReducer,
  },
});

export default store;
