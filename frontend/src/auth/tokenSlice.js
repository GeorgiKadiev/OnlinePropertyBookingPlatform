import { createSlice } from '@reduxjs/toolkit';

const tokenSlice = createSlice({
  name: 'authToken',
  initialState: null, // Initially no token
  reducers: {
    setToken: (state, action) => action.payload, // Save the token
    clearToken: () => null, // Clear the token on logout
  },
});

export const { setToken, clearToken } = tokenSlice.actions; // Export actions
export default tokenSlice.reducer; // Export reducer
