import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";
import { Box, TextField, Button, Typography } from "@mui/material";

export default function PropertyForm() {
  const navigate = useNavigate();
  const token = useSelector((state) => state.token); // Get token from Redux
  const [formData, setFormData] = useState({
    Location: "",
    Title: "",
    PricePerNight: "",
    Description: "",
  });
  const [error, setError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

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
    };

    // Validate form data
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
    console.log(token)

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

      //   setSuccessMessage("Property created successfully!");
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
        <Button variant="contained" color="primary" type="submit" fullWidth>
          Create Property
        </Button>
      </form>
    </Box>
  );
}
