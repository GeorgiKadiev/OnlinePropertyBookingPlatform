import { configureStore } from '@reduxjs/toolkit';
import tokenReducer from '../utils/tokenSlice';
import roleReducer from '../utils/roleSlice';
import idReducer from '../utils/idSlice';

const store = configureStore({
  reducer: {
    token: tokenReducer,
    role: roleReducer,
    id: idReducer,
  },
});

export default store;
