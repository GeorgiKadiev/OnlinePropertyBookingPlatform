import React, { useState } from "react";
import { useParams } from "react-router-dom";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  Box,
  TextField,
  Button,
  Typography,
  CircularProgress,
} from "@mui/material";
import "./AddRoomForm.css";
import NavBar from "../NavBar/NavBar";

export default function AddRoomForm() {
  const { id } = useParams();
  const token = useSelector((state) => state.token);
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    Name: "",
    Description: "",
    BedCount: "",
    MaxGuests: "",
  });
  const [message, setMessage] = useState(""); // For success messages
  const [error, setError] = useState(null); // For error messages

  // Handle input change
  const handleChange = (event) => {
    const { name, value } = event.target;

    setFormData((prevData) => ({
      ...prevData,
      [name]:
        name === "BedCount" || name === "MaxGuests"
          ? parseInt(value, 10)
          : value,
    }));
  };

  // Handle form submission
  const handleSubmit = async (event) => {
    event.preventDefault();
    setMessage("");
    setError(null);

    const payload = {
      Name: formData.Name,
      Description: formData.Description,
      BedCount: Number(formData.BedCount),
      MaxGuests: Number(formData.MaxGuests),
    };

    try {
      const response = await fetch(`http://localhost:5076/api/room/${id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        throw new Error("Failed to add room. Please try again.");
      }

      setFormData({
        Name: "",
        Description: "",
        BedCount: 0,
        MaxGuests: 0,
      });
      navigate("/landing-page", { state: { successMessage: "Room added successfully!" } });
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div>
      <NavBar />
      <Box className="add-room-form">
        <Typography variant="h5" className="form-title">
          Add Room
        </Typography>
        <form onSubmit={handleSubmit}>
          <TextField
            label="Room Name"
            name="Name"
            value={formData.Name}
            onChange={handleChange}
            fullWidth
            margin="normal"
            required
          />
          <TextField
            label="Description"
            name="Description"
            value={formData.Description}
            onChange={handleChange}
            fullWidth
            margin="normal"
            required
          />
          <TextField
            label="Bed Count"
            name="BedCount"
            type="number"
            value={formData.BedCount}
            onChange={handleChange}
            fullWidth
            margin="normal"
            required
          />
          <TextField
            label="Max Guests"
            name="MaxGuests"
            type="number"
            value={formData.MaxGuests}
            onChange={handleChange}
            fullWidth
            margin="normal"
            required
          />
          <Button
            type="submit"
            variant="contained"
            color="primary"
            className="submit-button"
          >
            Add Room
          </Button>
          {message && (
            <Typography className="success-message">{message}</Typography>
          )}
          {error && <Typography color="error">{error}</Typography>}
        </form>
      </Box>
    </div>
  );
}
