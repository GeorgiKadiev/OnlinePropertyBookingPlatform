import React, { useState } from "react";
import {
  TextField,
  Checkbox,
  FormControlLabel,
  Button,
  Typography,
  Box,
  Grid,
} from "@mui/material";

const Filters = ({ onFilterSubmit }) => {
  const [location, setLocation] = useState("");
  const [minPrice, setMinPrice] = useState("");
  const [maxPrice, setMaxPrice] = useState("");
  const [isEcoFriendly, setIsEcoFriendly] = useState(false);
  const [amenities, setAmenities] = useState([]);
  const [numberOfPersons, setNumberOfPersons] = useState("");

  const handleAmenityChange = (e) => {
    const { value, checked } = e.target;
    setAmenities((prev) =>
      checked ? [...prev, value] : prev.filter((amenity) => amenity !== value)
    );
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    onFilterSubmit({
      Location: location,
      MinPrice: minPrice ? parseFloat(minPrice) : null,
      MaxPrice: maxPrice ? parseFloat(maxPrice) : null,
      IsEcoFriendly: isEcoFriendly,
      Amenities: amenities,
      NumberOfPersons: numberOfPersons ? parseInt(numberOfPersons) : null,
    });
  };

  return (
    <Box component="form" onSubmit={handleSubmit} sx={{ padding: 3 }}>
      <Typography variant="h5" sx={{ marginBottom: 2 }}>
        Filters
      </Typography>
      <Grid container spacing={2}>
        <Grid item xs={12} sm={6}>
          <TextField
            label="Location"
            fullWidth
            value={location}
            onChange={(e) => setLocation(e.target.value)}
          />
        </Grid>
        <Grid item xs={6} sm={3}>
          <TextField
            label="Min Price"
            type="number"
            fullWidth
            value={minPrice}
            onChange={(e) => setMinPrice(e.target.value)}
          />
        </Grid>
        <Grid item xs={6} sm={3}>
          <TextField
            label="Max Price"
            type="number"
            fullWidth
            value={maxPrice}
            onChange={(e) => setMaxPrice(e.target.value)}
          />
        </Grid>
        <Grid item xs={12}>
          <FormControlLabel
            control={
              <Checkbox
                checked={isEcoFriendly}
                onChange={(e) => setIsEcoFriendly(e.target.checked)}
              />
            }
            label="Eco-Friendly"
          />
        </Grid>
        <Grid item xs={12}>
          <Typography>Amenities:</Typography>
          <FormControlLabel
            control={<Checkbox value="Wi-Fi" onChange={handleAmenityChange} />}
            label="Wi-Fi"
          />
          <FormControlLabel
            control={
              <Checkbox value="Parking" onChange={handleAmenityChange} />
            }
            label="Parking"
          />
          <FormControlLabel
            control={
              <Checkbox value="Swimming Pool" onChange={handleAmenityChange} />
            }
            label="Swimming Pool"
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            label="Number of Persons"
            type="number"
            fullWidth
            value={numberOfPersons}
            onChange={(e) => setNumberOfPersons(e.target.value)}
          />
        </Grid>
      </Grid>
      <Button
        type="submit"
        variant="contained"
        color="primary"
        sx={{ marginTop: 2 }}
      >
        Apply Filters
      </Button>
    </Box>
  );
};

export default Filters;
