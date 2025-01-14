import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";
import {
  Box,
  TextField,
  Button,
  Typography,
  Autocomplete,
  Chip,
} from "@mui/material";

export default function PropertyForm() {
  const navigate = useNavigate();
  const token = useSelector((state) => state.token); // Get token from Redux
  const [formData, setFormData] = useState({
    Location: "",
    Title: "",
    PricePerNight: "",
    Description: "",
    amenities: [], // Add this
  });
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const amenitiesOptions = [
    "Wi-Fi",
    "Parking",
    "Swimming Pool",
    "Eco-Friendly",
    "Hair Dryer",
    "Fridge",
    "Fireplace",
    "Air Conditioning",
    "Pet-Friendly",
    "Digital Nomad Friendly",
    "Coffee Maker",
    "Balcony",
    "Kitchen",
    "Fitness Centre",
  ];

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      Location: formData.Location,
      Title: formData.Title,
      PricePerNight: Number(formData.PricePerNight),
      Description: formData.Description,
      // Amenity: formData.amenities, // Include amenities
    };

    if (
      !formData.Location ||
      !formData.Title ||
      !formData.PricePerNight ||
      !formData.Description
    ) {
      setError("All fields are required");
      return;
    }

    console.log(payload);

    try {
      const response = await fetch("http://localhost:5076/api/estate/create", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`, // Add token for authentication
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        throw new Error(
          `Failed to create property: ${response.status} ${response.statusText}`
        );
      }

      navigate("/success");
    } catch (error) {
      setError("Error creating property: " + error.message);
      setSuccessMessage("");
    }
  };

  return (
    <Box
      sx={{
        maxWidth: "400px",
        margin: "0 auto",
        padding: "20px",
        display: "flex",
        flexDirection: "column",
        gap: "16px",
        border: "1px solid #ccc",
        borderRadius: "8px",
        boxShadow: "0px 2px 4px rgba(0, 0, 0, 0.1)",
      }}
    >
      <Typography variant="h5" align="center">
        Create Property
      </Typography>

      {error && <Typography color="error">{error}</Typography>}
      {successMessage && (
        <Typography color="primary">{successMessage}</Typography>
      )}

      <form onSubmit={handleSubmit}>
        <TextField
          label="Location"
          name="Location"
          value={formData.Location}
          onChange={handleChange}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Title"
          name="Title"
          value={formData.Title}
          onChange={handleChange}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Price Per Night"
          name="PricePerNight"
          type="number"
          value={formData.PricePerNight}
          onChange={handleChange}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Description"
          name="Description"
          value={formData.Description}
          onChange={handleChange}
          fullWidth
          margin="normal"
        />
      
        {/* <Autocomplete
          multiple
          options={amenitiesOptions}
          getOptionLabel={(option) => option}
          value={formData.amenities}
          onChange={(event, value) =>
            setFormData({ ...formData, amenities: value })
          }
          renderTags={(value, getTagProps) =>
            value.map((option, index) => (
              <Chip key={index} label={option} {...getTagProps({ index })} />
            ))
          }
          renderInput={(params) => (
            <TextField
              {...params}
              variant="outlined"
              label="Amenities"
              placeholder="Select amenities"
              margin="normal"
            />
          )}
          fullWidth
        /> */}
        <Button variant="contained" color="primary" type="submit" fullWidth>
          Create Property
        </Button>
      </form>
    </Box>
  );
}
