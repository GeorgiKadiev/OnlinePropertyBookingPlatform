import { createSlice } from '@reduxjs/toolkit';

const idSlice = createSlice({
  name: 'userId',
  initialState: null, 
  reducers: {
    setId: (state, action) => action.payload, 
    clearId: () => null,
  },
});

export const { setId, clearId } = idSlice.actions; // Export actions
export default idSlice.reducer; // Export reducer
