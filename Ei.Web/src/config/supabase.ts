import { createClient } from "@supabase/supabase-js";

const supabaseUrl = "https://lfnepyttgazupgzgioue.supabase.co"; //  process.env.REACT_APP_SUPABASE_URL;
const supabaseAnonKey =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiYW5vbiIsImlhdCI6MTYzMTUwMjA4MiwiZXhwIjoxOTQ3MDc4MDgyfQ.5P75IWkKGL_RnobwAIKD3QSHt7yg2el5RtYBQSi6taw";

export const supabase = createClient(supabaseUrl, supabaseAnonKey);
