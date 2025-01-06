import * as React from "react";
import {
  Box,
  Button,
  Chip,
  TextField,
  Autocomplete,
} from "@mui/material";
import "./FiltersSideBar.css";

// Sample data for Autocomplete
const top100Films = [
  { title: "The Shawshank Redemption" },
  { title: "The Godfather" },
  { title: "The Dark Knight" },
  { title: "Pulp Fiction" },
  { title: "The Lord of the Rings" },
  { title: "Forrest Gump" },
  { title: "Inception" },
  { title: "Fight Club" },
  { title: "The Matrix" },
];

export default function FiltersSideBar() {
  return (
    <Box className="filters-sidebar">
      <h3>Filter Options</h3>

      {/* Autocomplete Input */}
      <Autocomplete
        multiple
        id="tags-filled"
        options={top100Films.map((option) => option.title)}
        freeSolo
        className="autocomplete-input"
        renderTags={(value, getTagProps) =>
          value.map((option, index) => {
            const { key, ...tagProps } = getTagProps({ index });
            return (
              <Chip variant="outlined" label={option} key={key} {...tagProps} />
            );
          })
        }
        renderInput={(params) => (
          <TextField
            {...params}
            variant="outlined"
            label="Tags"
            placeholder="Add tags"
          />
        )}
      />

      {/* Filter Button */}
      <Button variant="contained" color="primary" className="apply-button">
        Apply Filters
      </Button>
    </Box>
  );
}
