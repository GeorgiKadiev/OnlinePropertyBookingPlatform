import * as React from "react";
import MenuItem from "@mui/material/MenuItem";
import MenuList from "@mui/material/MenuList";
import Divider, { dividerClasses } from "@mui/material/Divider";
import Box from '@mui/material/Box';

export default function MenuListComposition() {
  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        [`& .${dividerClasses.root}`]: {
          mx: 0.5,
        },
      }}
    >
      <MenuList>
        <MenuItem>All reservations</MenuItem>
        <MenuItem>Current reservations</MenuItem>
        <MenuItem>Past reservations</MenuItem>
        <MenuItem>Future reservations</MenuItem>
      </MenuList>
      <Divider orientation="vertical" flexItem />
    </Box>
  );
}
