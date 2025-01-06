import { configureStore } from '@reduxjs/toolkit';
import tokenReducer from '../auth/tokenSlice';
import roleReducer from '../auth/roleSlice';

const store = configureStore({
  reducer: {
    token: tokenReducer,
    role: roleReducer,
  },
});

export default store;
