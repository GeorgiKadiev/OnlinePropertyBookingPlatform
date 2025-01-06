import { configureStore } from '@reduxjs/toolkit';
import tokenReducer from '../auth/tokenSlice';

const store = configureStore({
  reducer: {
    token: tokenReducer,
  },
});

export default store;
