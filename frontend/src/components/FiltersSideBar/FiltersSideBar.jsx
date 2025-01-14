import React, { useState } from "react";
import { Box, Button, Chip, TextField, Autocomplete } from "@mui/material";
import { setFilters } from "../../utils/filterSlice";
import "./FiltersSideBar.css";

const amenities = [
  { name: "Wi-Fi" },
  { name: "Parking" },
  { name: "Swimming Pool" },
  { name: "Eco-Friendly" },
  { name: "Hair Dryer" },
  { name: "Fridge" },
  { name: "Fireplace" },
  { name: "Air Conditioning" },
  { name: "Pet-Friendly" },
  { name: "Digital Nomad Friendly" },
  { name: "Coffe Maker" },
  { name: "Balcony" },
  { name: "Kitchen" },
  { name: "Fitness Centre" },
];

export default function FiltersSideBar() {
  const [selectedAmenities, setSelectedAmenities] = useState([]);

  const handleApplyFilters = async () => {
    const payload = { amenities: selectedAmenities };
    setFilters(payload);
    console.log(payload);
  };

  return (
    <Box className="filters-sidebar">
      <h3>Filter Options</h3>

      {/* Autocomplete Input */}
      <Autocomplete
        multiple
        id="tags-filled"
        options={amenities.map((option) => option.name)}
        freeSolo
        onChange={(event, value) => setSelectedAmenities(value)}
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
      <Button
        variant="contained"
        color="primary"
        className="apply-button"
        onClick={handleApplyFilters} // Call API on button click
        disabled={selectedAmenities.length === 0} // Disable if no tags are selected
      >
        Apply Filters
      </Button>
    </Box>
  );
}
